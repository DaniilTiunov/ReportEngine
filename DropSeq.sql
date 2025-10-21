SELECT setval(
    pg_get_serial_sequence('"Companies"', 'Id'),
    COALESCE((SELECT MAX("Id") FROM "Companies"), 1)
);