CREATE TABLE IF NOT EXISTS users (
    id              UUID            PRIMARY KEY,
    email           VARCHAR(320)    NOT NULL,
    password_hash   VARCHAR(1024)   NOT NULL,
    created_at      TIMESTAMPTZ     NOT NULL DEFAULT now(),

    CONSTRAINT uq_users_email UNIQUE (email)
);

CREATE TABLE IF NOT EXISTS tasks (
    id              UUID            PRIMARY KEY,
    owner_id        UUID            NOT NULL,
    title           VARCHAR(200)    NOT NULL,
    description     TEXT,
    status          SMALLINT        NOT NULL DEFAULT 0,
    due_date        TIMESTAMPTZ,
    created_at      TIMESTAMPTZ     NOT NULL DEFAULT now(),
    updated_at      TIMESTAMPTZ     NOT NULL DEFAULT now(),

    CONSTRAINT fk_tasks_owner FOREIGN KEY (owner_id) REFERENCES users (id) ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS idx_tasks_owner_id ON tasks (owner_id);
