INSERT INTO users (id, email, password_hash, created_at)
VALUES (
    'a1b2c3d4-e5f6-7890-abcd-ef1234567890',
    'demo@ballastlane.com',
    'AQAAAAIAAYagAAAAEAb/N9haFG/JpYpluaUc96sHJuPyjeIz6FCc/29Yfci/+DUX6qAPHA140J7o4sAnSw==',
    now()
)
ON CONFLICT (id) DO NOTHING;

INSERT INTO tasks (task_id, owner_id, title, description, status, due_date, created_at, updated_at)
VALUES
    ('b1000001-0000-0000-0000-000000000001', 'a1b2c3d4-e5f6-7890-abcd-ef1234567890', 'Review project proposal', 'Analyze the new project proposal and provide feedback to the team.', 0, '2026-05-10T12:00:00Z', '2026-05-01T09:00:00Z', '2026-05-01T09:00:00Z'),
    ('b1000002-0000-0000-0000-000000000002', 'a1b2c3d4-e5f6-7890-abcd-ef1234567890', 'Set up CI pipeline', 'Configure GitHub Actions for automated builds and test runs.', 0, '2026-05-18T12:00:00Z', '2026-05-01T10:30:00Z', '2026-05-01T10:30:00Z'),
    ('b1000003-0000-0000-0000-000000000003', 'a1b2c3d4-e5f6-7890-abcd-ef1234567890', 'Write unit tests for auth module', 'Cover registration, login, and token validation flows.', 1, '2026-05-08T12:00:00Z', '2026-05-02T08:15:00Z', '2026-05-03T14:00:00Z'),
    ('b1000004-0000-0000-0000-000000000004', 'a1b2c3d4-e5f6-7890-abcd-ef1234567890', 'Update API documentation', 'Refresh the OpenAPI spec and Scalar UI annotations.', 1, NULL, '2026-05-02T16:45:00Z', '2026-05-03T11:20:00Z'),
    ('b1000005-0000-0000-0000-000000000005', 'a1b2c3d4-e5f6-7890-abcd-ef1234567890', 'Fix login page styling', 'Resolve alignment issues on the login form for mobile viewports.', 2, '2026-05-02T12:00:00Z', '2026-05-03T09:00:00Z', '2026-05-04T10:30:00Z')
ON CONFLICT (task_id) DO NOTHING;
