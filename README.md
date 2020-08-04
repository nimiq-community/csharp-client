Nimiq C# Client
===============

> C# implementation of the Nimiq RPC client specs.

## Usage

Send requests to a Nimiq node with `NimiqClient` object.

```c#
var config = new Config(
    scheme: "http",
    host: "127.0.0.1",
    port: 8648,
    user: "luna",
    password: "moon"
);

var client = new NimiqClient(config)
```

Once the client have been set up, we can call the methods with the appropiate arguments to make requests to the Nimiq node.

When no `config` object is passed in the initialization it will use the default values in the Nimiq node.

```c#
var client = NimiqClient();

// make rpc call to get the block number
var blockNumber = await client.blockNumber();

Console.WriteLine(blockNumber) // displays the block number, for example 748883
```

## API

The complete [API documentation](docs) is available in the `/docs` folder.

Check out the [Nimiq RPC specs](https://github.com/nimiq/core-js/wiki/JSON-RPC-API) for behind the scene RPC calls.

## Installation

To install NimiClient add the Nuget package as a dependency to your project from Visual Studio IDE.

## Contributions

This implementation was originally contributed by [rraallvv](https://github.com/rraallvv/).

Please send your contributions as pull requests.

Refer to the [issue tracker](https://github.com/nimiq-community/csharp-client/issues) for ideas.

### Develop

After cloning the repository, open the solucion file `NimiqClientProjects.sln` located in the repository root folder on Visual Studio.

All done, happy coding!

### Testing

Tests are stored in the `/NimiqClientTest` folder and can be run from the Visual Studio IDE.

### Documentation

The documentation is generated automatically running [Doxygen](https://www.doxygen.nl/download.html#srcbin) from the repository root directory.

```sh
doxygen doxygenfile
```

## License

[Apache 2.0](LICENSE.md)
