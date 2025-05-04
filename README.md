# ParentsApp – Formularz dla rodziców (.NET 8 MVC)

Aplikacja webowa umożliwia dodawanie informacji o rodzicach (Mama/Tata) poprzez formularz. Dane są zapisywane asynchronicznie do pliku tekstowego, z walidacją i kontrolą unikalności (imię + nazwisko).

# Funkcje:

1. Formularz dodawania rodzica z losowym pytaniem (spośród 100).
2. Walidacja po stronie klienta i serwera.
3. Sprawdzenie unikalności rodzica (bez względu na wielkość liter).
4. Asynchroniczny zapis do pliku `rodzice.txt`.
5. Lista zapisanych rodziców z estetycznym wyglądem.
6. Rejestrowanie błędów w pliku `error-log.txt`.

# Technologie:

* ASP.NET Core 8 MVC (Razor)
* Bootstrap 5
* AJAX 
