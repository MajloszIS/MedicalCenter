---
_layout: landing
---

# MedicalCenter

Aplikacja webowa centrum medycznego zbudowana w **ASP.NET Core MVC (.NET 9)** z modułem sprzedaży leków, obsługą dostaw oraz pełną dokumentacją API.

## O projekcie

MedicalCenter to kompleksowy system informatyczny dla placówki medycznej, obejmujący panele dla czterech ról użytkowników: administratora, lekarza, pacjenta oraz kuriera. Aplikacja łączy funkcje typowego systemu medycznego (umawianie wizyt, dokumentacja medyczna, recepty) z modułem e-commerce do sprzedaży leków.

## Główne funkcjonalności

- **Rejestracja i logowanie** — klasyczne konto z hasłem lub przez Google OAuth
- **Panel pacjenta** — umawianie wizyt, podgląd diagnoz, recept i zwolnień, zakup leków
- **Panel lekarza** — zarządzanie wizytami, wystawianie diagnoz, recept, skierowań i zwolnień
- **Panel administratora** — zarządzanie użytkownikami, lekami, departamentami i specjalizacjami
- **Panel kuriera** — realizacja dostaw zamówień
- **Sklep z lekami** — koszyk, płatności Stripe (karty, BLIK, Przelewy24)
- **Generowanie dokumentów PDF** — recepty z QR kodem (QuestPDF)
- **REST API** — z autoryzacją JWT i dokumentacją Swagger

## Stack technologiczny

| Warstwa            | Technologia                                |
|--------------------|--------------------------------------------|
| Backend            | ASP.NET Core MVC 9.0                       |
| ORM                | Entity Framework Core 9                    |
| Baza danych        | SQL Server (lokalnie Docker, prod. Azure)  |
| Frontend           | Razor Views, Bootstrap 5, Font Awesome     |
| Autoryzacja        | Cookie Authentication + JWT                |
| Płatności          | Stripe                                     |
| Logowanie zewn.    | Google OAuth 2.0                           |
| Generowanie PDF    | QuestPDF                                   |
| Hosting            | Azure App Service + Azure SQL Database     |

## Dokumentacja

- [Getting Started](/_site/docs/getting-started.html) — instrukcja uruchomienia lokalnie
- [README projektu](/_site/README.html) — pełny opis projektu i architektury
- [API Reference](/_site/api/MedicalCenter.Controllers.html) — dokumentacja klas i metod

## Repozytorium

Kod źródłowy dostępny na GitHubie: [github.com/MajloszIS/MedicalCenter](https://github.com/MajloszIS/MedicalCenter)