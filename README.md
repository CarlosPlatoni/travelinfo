# TravelInfo
Travel Information and Photoframe project for Windows IOT

Universal Windows Application written in C# designed for a Raspberry Pi running Windows IOT with the official Raspberry Pi foundation screen. Connects to both National Rail and TFL datafeeds to get National Rail train times, bus times and London Tube status information. This could be used as the basis of your own project as the stations and tube lines are hardcoded for my local stations. Requires your own account on both the National Rail datafeed and the TFL datafeed to get the information. As there is a monthly limit to the number of calls that can be made to National Rail, the code will only fetch the data for a set time period in the morning. It acts as a photoframe when not displaying train times. The photos reside on my server and a very simple photo server is included in the project. Again this is hard coded for my use providing random photos, but could easily be changed to provide photos from your own source.
