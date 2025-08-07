-- Установите последовательность на максимальное значение Id в таблице, уменьшенное на 1
ALTER SEQUENCE "Projects_Id_seq" MINVALUE 0;
SELECT setval('"Projects_Id_seq"', 0);

