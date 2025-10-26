using Npgsql;
using NpgsqlTypes;
using Survey.Domain;

namespace Survey.Persistence
{
    public class SurveyRepository
    {
        private readonly string statsSql = @"
                WITH qmax AS (
                  VALUES
                    ('question1', 9), ('question2', 7),
                    ('question3', 4), ('question4', 4), ('question5', 4),
                    ('question6', 4), ('question7', 4), ('question8', 4),
                    ('question9', 4), ('question10',4), ('question11',4),
                    ('question12',4), ('question13',4), ('question14',4),
                    ('question15',4)
                ),
                agg AS (
                  SELECT t.question, t.value::int AS value, count(*) AS cnt
                  FROM surveis s
                  CROSS JOIN LATERAL (VALUES
                    ('question1',  s.question1),
                    ('question2',  s.question2),
                    ('question3',  s.question3),
                    ('question4',  s.question4),
                    ('question5',  s.question5),
                    ('question6',  s.question6),
                    ('question7',  s.question7),
                    ('question8',  s.question8),
                    ('question9',  s.question9),
                    ('question10', s.question10),
                    ('question11', s.question11),
                    ('question12', s.question12),
                    ('question13', s.question13),
                    ('question14', s.question14),
                    ('question15', s.question15)
                  ) AS t(question, value)
                  WHERE t.value IS NOT NULL
                  GROUP BY t.question, t.value
                )
                SELECT q.question,
                       gs.val AS value,
                       COALESCE(a.cnt, 0) AS cnt
                FROM qmax AS q(question, maxv)
                CROSS JOIN LATERAL generate_series(1, q.maxv) AS gs(val)
                LEFT JOIN agg a ON a.question = q.question AND a.value = gs.val
                ORDER BY q.question, gs.val;
                ";

        const string insertSql = @"
            INSERT INTO surveis (
              id,
              question1, question2, question3, question4, question5,
              question6, question7, question8, question9, question10,
              question11, question12, question13, question14, question15,
              opinion, created_on
            ) VALUES (
              @id,
              @q1, @q2, @q3, @q4, @q5,
              @q6, @q7, @q8, @q9, @q10,
              @q11, @q12, @q13, @q14, @q15,
              @opinion, @created_on
            );";

        private readonly NpgsqlDataSource _dataSource;
        public SurveyRepository(NpgsqlDataSource dataSource)
        {
            _dataSource = dataSource;
        }

        public async Task<SurveyStats> GetSurveyStats()
        {
            var result = new Dictionary<int, Dictionary<int, int>>();

            await using var cmd = _dataSource.CreateCommand(statsSql);
            await using var reader = await cmd.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var qName = reader.GetString(0);
                var val = reader.GetInt32(1);
                var cnt = reader.GetFieldValue<long>(2);

                if (!qName.StartsWith("question", StringComparison.OrdinalIgnoreCase)
                    || !int.TryParse(qName.Substring("question".Length), out var qNumber))
                {
                    continue;
                }

                if (!result.TryGetValue(qNumber, out var inner))
                {
                    inner = new Dictionary<int, int>();
                    result[qNumber] = inner;
                }

                inner[val] = checked((int)cnt);
            }

            return new SurveyStats() { Questions = result };
        }

        public async Task InsertSurvey(SurveyResult result)
        {
            await using var cmd = _dataSource.CreateCommand(insertSql);
            cmd.Parameters.Add(new NpgsqlParameter("id", NpgsqlDbType.Uuid) { Value = Guid.NewGuid() });

            for (int i = 1; i <= 15; i++)
            {
                var paramName = $"q{i}";
                var answerValue = result.Answers[i];
                cmd.Parameters.Add(new NpgsqlParameter(paramName, NpgsqlDbType.Integer) { Value = answerValue });
            }

            cmd.Parameters.Add(new NpgsqlParameter("opinion", NpgsqlDbType.Text) { Value = (object?)result.Opinion ?? DBNull.Value });
            cmd.Parameters.Add(new NpgsqlParameter("created_on", NpgsqlDbType.Timestamp) { Value = DateTime.Now });

            await cmd.ExecuteNonQueryAsync();
        }
    }
}

