CryptoWallet API

Простое API на ASP.NET Core для эмуляции кастодиального криптовалютного кошелька.

Запуск без Docker :

git clone https://github.com/milkweed123/cryptowallet.git

cd cryptowallet

Поменять в appsettings.json путь к бд, применить миграции с помощью dotnet ef database update

dotnet run --project CryptoWallet.Api
