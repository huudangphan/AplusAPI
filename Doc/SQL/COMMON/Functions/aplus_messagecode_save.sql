create or replace function aplus_messagecode_save() returns int
    language plpgsql as
$$

begin
    -- update message from client request_body
    insert into message_system (select * from _client_msgs)
    on conflict on constraint message_system_pk do update set msg = excluded.msg;

    -- update system message code from system to existed message
    insert into message_system (select * from _system_msgs) on conflict do nothing;

    return 200;
end
$$;





