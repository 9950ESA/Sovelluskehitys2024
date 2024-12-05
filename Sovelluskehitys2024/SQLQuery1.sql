
CREATE TABLE kirjat (id INTEGER IDENTITY(1,1) PRIMARY KEY, nimi VARCHAR(50), vuosi INTEGER, tekija VARCHAR(50));
CREATE TABLE asiakkaat (id INTEGER IDENTITY(1,1) PRIMARY KEY, nimi VARCHAR(50), osoite VARCHAR(150), puhelin VARCHAR(50));
CREATE TABLE kopiot (id INTEGER IDENTITY(1,1) PRIMARY KEY, kirja_id INTEGER REFERENCES kirjat(id) ON DELETE CASCADE, maara INTEGER);
CREATE TABLE lainat (id INTEGER IDENTITY(1,1) PRIMARY KEY, asiakas_id INTEGER REFERENCES asiakkaat(id) ON DELETE CASCADE, kopio_id INTEGER REFERENCES kopiot(id) ON DELETE CASCADE, haettu BIT DEFAULT 0);

INSERT INTO kirjat (nimi, vuosi, tekija) VALUES ('Python-alkeet', 2020, 'Matti Meikälainen');
INSERT INTO kirjat (nimi, vuosi, tekija) VALUES ('SQL-perusteet', 2021, 'Mikki Hiiri');
INSERT INTO kirjat (nimi, vuosi, tekija) VALUES ('Aku Ankan taskukirja 17', 1966, 'Aku Ankka');
INSERT INTO asiakkaat (nimi, osoite, puhelin) VALUES ('Masa', 'Kuusikuja 6', '050882682');
INSERT INTO asiakkaat (nimi, osoite, puhelin) VALUES ('Maija', 'Kampusranta 11', '040345774');
INSERT INTO kopiot (kirja_id, maara) VALUES (1,3);
INSERT INTO kopiot (kirja_id, maara) VALUES (2,3);
INSERT INTO kopiot (kirja_id, maara) VALUES (3,3);
INSERT INTO lainat (asiakas_id, kopio_id) VALUES (2,1); 
INSERT INTO lainat (asiakas_id, kopio_id) VALUES (1,2);
INSERT INTO lainat (asiakas_id, kopio_id, haettu) VALUES (1,3,1); 

SELECT l.id AS id, a.nimi AS asiakas_nimi, k.nimi AS kirja_nimi, ko.kirja_id, l.haettu AS laina_haettu FROM asiakkaat a, lainat l, kopiot ko, kirjat k WHERE a.id = l.asiakas_id AND l.kopio_id = ko.id AND ko.kirja_id = k.id AND l.haettu='0';
SELECT l.id AS id, a.nimi AS asiakas_nimi, k.nimi AS kirja_nimi, ko.kirja_id, l.haettu AS laina_haettu FROM asiakkaat a, lainat l, kopiot ko, kirjat k WHERE a.id = l.asiakas_id AND l.kopio_id = ko.id AND ko.kirja_id = k.id AND l.haettu='1';

SELECT k.*, (SELECT COUNT(*) FROM lainat l WHERE l.kopio_id = k.kirja_id) AS Lainassa FROM kopiot k;

DELETE FROM kirjat WHERE id=1;
DELETE FROM kirjat WHERE nimi="Python-alkeet";

SELECT * FROM kirjat;
SELECT * FROM asiakkaat;
SELECT * FROM kopiot;
SELECT * FROM lainat;

DROP TABLE lainat;
DROP TABLE kopiot;
DROP TABLE kirjat;
DROP TABLE asiakkaat;
