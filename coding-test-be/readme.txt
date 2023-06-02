1- Build & run the app. Hit the 'update customer name' endpoint:

PUT /customers/1

Request body:
{
	"name": "George"
}

The call will fail.

1-A.	Find a way to fix the errors till the endpoint successfully updates a customer name
1-B.	If the customer ID parameter doesn't match any existing record, return a proper HTTP error code and log a Warning message in the console.

2- Create an endpoint in the ProductsController that returns all the products sorted by highest price first, then alphabetically by Name. The endpoint URI should be '/products/most-expensive'

3- Create an endpoint 'customers/top' that returns the top five customers who spent the most between 1/1/2015 and 31/12/2022 (inclusive range), sorted by total spending in descending order. Put some of the filtering/sorting logic in the controller or in a new service/query, not only in the repositories.

4- Add unit tests that cover different scenarios for the logic created in point 3-. Mock the repository and any other dependencies with NSubstitute, handle the tests expectations with FluentAssertions.

5- Push to a public repo on GitHub and send the link

Optional - nice to have:

6- Protect the endpoints requiring an API key in the HTTP header sent to the server. Read the expected API key from the appsettings.json file. Use the https://github.com/mihirdilip/aspnetcore-authentication-apikey Nuget package.