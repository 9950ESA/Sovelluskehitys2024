CREATE TABLE tuotteet (id INTEGER IDENTITY(1,1) PRIMARY KEY, nimi VARCHAR(50), hinta INTEGER);

INSERT INTO tuotteet (nimi, hinta) VALUES ('juusto', 6);

SELECT * FROM tuotteet;

DELETE FROM tuotteet WHERE nimi="kinkku";

DROP TABLE tuotteet;