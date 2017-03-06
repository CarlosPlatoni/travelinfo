# TravelInfo
## Travel Information and Photoframe project for Windows IOT

Universal Windows Application written in C# and XAML designed for a Raspberry Pi running Windows IOT with the official Raspberry Pi foundation screen. As it is a UWA the code can be used for Windows Phones and Windows 10 Store Apps.

Connects to and displays on one screen
- National Rail Darwin Feed to get National Rail times (Account required)
- TFL unified API to get bus times and London Tube status information (Account required). 

This could be used as the basis of your own project. Accounts on TFL and National Rail are free. The project is fully working for a set of stations and bus stops in South London. In the case of National Rail a monthly limit is imposed, so the code will only fetch the data for a set time period in the morning. 

It acts as a photoframe when not displaying train times. The photos reside on an internal server and a very simple photo WCF server is included in the project. 
