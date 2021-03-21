create table feedbacks (
	id bigserial primary key,
	create_time timestamptz not null default now(),
	user bigint references users (id),
	feedback text not null
);
create index feedbacks_create_time_idx ON feedbacks (create_time);
create index feedbacks_user_idx ON feedbacks (user);