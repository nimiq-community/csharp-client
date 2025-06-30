# Nimiq C# Client

> C# implementation of the Nimiq RPC client specs.

This repository is archived: Nimiq PoS has been launched and this RPC client only supports the
old PoW RPC specification. As of now, there is no C# RPC client implementation. Please
[contact us](mailto:community@nimiq.com) if you are interested in implementing and supporting the
Nimiq ecosystem for C#.

## Usage

To get started sending requests to a Nimiq node, we create a `NimiqClient` object.

```c#
var config = new Config(
    scheme: "http",
    host: "127.0.0.1",
    port: 8648,
    user: "luna",
    password: "moon"
);

var client = new NimiqClient(config);

// make rpc call to get current block number
var blockNumber = await client.blockNumber();
Console.WriteLine(blockNumber);
```

Note: When no `config` object is passed in the initialization it will use the default values in the Nimiq node.

## API

The complete API documentation is available [here](https://nimiq-community.github.io/csharp-client/).

Check out the original [Nimiq RPC specs](https://github.com/nimiq/core-js/wiki/JSON-RPC-API) for the behind-the-scenes RPC calls.

## Installation

Use the Package Manager from Visual Studio IDE to install the `NimiqClient` package as a dependency to your project.

Alternatively, you can install the package from the project folder using `dotnet` [from the command line](https://docs.microsoft.com/en-us/nuget/consume-packages/install-use-packages-dotnet-cli):

```sh
dotnet add package NimiqClient
```

## Build

After cloning the repository, open the solution file `NimiqClientProjects.sln` located in the repository root folder in Visual Studio.

All done, happy coding!

## Test

You need a start a Testnet Nimiq node:

```sh
nodejs index.js --protocol=dumb --type=full --network=test --rpc
```

The tests are stored in `\NimiqClientTest` and can be run from the Visual Studio IDE. Alternatively you can run all the tests from the command line from the repository root directory:

```sh
dotnet test
```

## Documentation

The documentation is generated automatically with [DocFX](https://dotnet.github.io/docfx/). To create the documentation, run the following commands from the repository root directory.

First install the DocFX package:

```sh
nuget install docfx.console -version 2.51.0
```

Copy the file `README.md` to the same folder with the name `index.md`:

(Windows)

```sh
copy /y README.md index.md
```

(Unix-like)

```sh
cp -r README.md index.md
```

Remove the old generated documentation:

(Windows)

```sh
rmdir /s /q docs
```

(Unix-like)

```sh
rm -fr docs
```

Run DocFX with the configuration file `docfx.json` via the mono command:

```sh
mono docfx.console.2.51.0/tools/docfx.exe docfx.json
```

Note: Mono is distributed with the .Net SDK, if it isn't already installed on your system download and install the tool from [here](https://www.mono-project.com/docs/tools+libraries/tools/).

Finally add a blank file in the `\docs` folder with the name `.nojekyll` for the documentation hosted on GitHub Pages:

(Windows)

```sh
echo > docs\.nojekyll
```

(Unix-like)

```sh
echo > docs/.nojekyll
```

## Contributions

This implementation was originally contributed by [rraallvv](https://github.com/rraallvv/).

Bug reports and pull requests are welcome! Please refer to the [issue tracker](https://github.com/nimiq-community/csharp-client/issues) for ideas.

## License

[Apache 2.0](LICENSE)
