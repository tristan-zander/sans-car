# Sans Car
Discord bot and website.

## Dependencies
* PostgreSQL
* Dotnet Runtime
* Docker (optional)
* Lavalink (optional)
   * Get the latest release from Lavalink's Github repository.
   * Copy the `application.yml` file to the location that you'll be running LavaLink.

## Setup
1. Copy "config.example.json" into "config.json"
   * Edit values to your needs, such as the bot token or enabling LavaLink.
2. Create a PostgreSQL Database and copy its values into config.json
   * In order to properly format the database, run the following commands: 
   ```bash
   $ cd Website
   $ dotnet tool install --global dotnet-ef
   $ dotnet ef migrations add InitialCreate
   $ dotnet ef database update
   ``` 
   **NOTE: Don't forget to add a new database migration after every update.** This way of managing the
   database might become cumbersome and I may change it to raw sql later.
   * To use SQLite or SQLServer, change the default Entity Framework service in 
   Website/Startup.cs and change your connection string. This may involve adding new NuGet packages.
3. Copy the service files to `/etc/systemd/system` if you plan to run it as a system service.
4. Run the bot.
   * Run as system service: `systemctl enable --now sans-car.service`
   * Run in a shell: `./run_bot.sh` or `./run_website.sh`
   * Note: I will not be doing any testing of the service on Windows, but it should be just as simple
   to get it running. Just copy the steps in the run_*.sh files in a command prompt.
