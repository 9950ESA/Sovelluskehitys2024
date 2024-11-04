CREATE TABLE tuotteet (id INTEGER IDENTITY(1,1) PRIMARY KEY, nimi VARCHAR(50), hinta INTEGER);
CREATE TABLE asiakkaat (id INTEGER IDENTITY(1,1) PRIMARY KEY, nimi VARCHAR(50), osoite VARCHAR(100), puhelin VARCHAR(50));
CREATE TABLE tilaukset (id INTEGER IDENTITY(1,1) PRIMARY KEY, asiakas_id INTEGER REFERENCES asiakkaat ON DELETE CASCADE, tuote_id INTEGER REFERENCES tuotteet ON DELETE CASCADE);

INSERT INTO tuotteet (nimi, hinta) VALUES ('makkara', 10);
INSERT INTO asiakkaat (nimi, osoite, puhelin) VALUES ('Matti', 'Kampusranta 11' ,'040 12345678');
INSERT INTO tilaukset (asiakas_id, tuote_id) VALUES (1,5);

SELECT * FROM tuotteet;
SELECT * FROM asiakkaat;
SELECT * FROM tilaukset;

DELETE FROM asiakkaat WHERE nimi="Matti";

DROP TABLE tilaukset;