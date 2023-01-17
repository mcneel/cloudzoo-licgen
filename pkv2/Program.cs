using PartialKeyVerification;
using PartialKeyVerification.Checksum;
using PartialKeyVerification.Hash;

// generate a new guid as the serial number
var id = Guid.NewGuid().ToString();

Console.WriteLine($"id: {id}");

// TODO: customise checksum, hash and base keys and then comment out the error below
#error Do not use without customising the checksum, hash and base keys!
var generator = new PartialKeyGenerator(new Adler16(), new Jenkins96(), new uint[] { 1, 2, 3, 4 }) { Spacing = 4 };

// generate license key
var key = generator.Generate(id);

// add prefix (required to identify product in cloud zoo)
var prefix = "ABC1";

key = prefix + "-" + key;

Console.WriteLine($"key: {key}");