INSERT INTO users (id, email, password_hash, created_at)
VALUES (
    'a1b2c3d4-e5f6-7890-abcd-ef1234567890',
    'demo@ballastlane.com',
    'AQAAAAIAAYagAAAAEOpKHtg0dLEzUHN4/AJOATy+bOIR1OJeKmCvZbBBI4YmPVL3VDChB4QAVWm//0C1zg==',
    now()
)
ON CONFLICT (id) DO NOTHING;

INSERT INTO tasks (id, owner_id, title, description, status, due_date, created_at, updated_at)
VALUES
    ('b1000001-0000-0000-0000-000000000001', 'a1b2c3d4-e5f6-7890-abcd-ef1234567890', 'Review project proposal', 'Analyze the new project proposal and provide feedback to the team.', 0, now() + INTERVAL '7 days', now(), now()),
    ('b1000002-0000-0000-0000-000000000002', 'a1b2c3d4-e5f6-7890-abcd-ef1234567890', 'Set up CI pipeline', 'Configure GitHub Actions for automated builds and test runs.', 0, now() + INTERVAL '14 days', now(), now()),
    ('b1000003-0000-0000-0000-000000000003', 'a1b2c3d4-e5f6-7890-abcd-ef1234567890', 'Write unit tests for auth module', 'Cover registration, login, and token validation flows.', 1, now() + INTERVAL '5 days', now(), now()),
    ('b1000004-0000-0000-0000-000000000004', 'a1b2c3d4-e5f6-7890-abcd-ef1234567890', 'Update API documentation', 'Refresh the OpenAPI spec and Scalar UI annotations.', 1, NULL, now(), now()),
    ('b1000005-0000-0000-0000-000000000005', 'a1b2c3d4-e5f6-7890-abcd-ef1234567890', 'Fix login page styling', 'Resolve alignment issues on the login form for mobile viewports.', 2, now() - INTERVAL '2 days', now(), now())
ON CONFLICT (id) DO NOTHING;
