// generate a new guid as the serial number
var id = Guid.NewGuid();

Console.WriteLine($"id: {id}");

// get an int hash of the guid
var seed = id.GetHashCode(); // TODO: don't rely on .net for the hash

Console.WriteLine($"seed: {seed} ({(uint)seed})");

// generate license key
var key = PKV_MakeKey(seed); // TODO: customise!

// add prefix (required to identify product in cloud zoo)
var prefix = "ABC1";

key = prefix + "-" + key;

Console.WriteLine($"key: {key}");

// Ported from the original blog post by Brandon Staggs [1], using Keygen's [2]
// node.js version to compare results.

// [1]: https://www.brandonstaggs.com/2007/07/26/implementing-a-partial-serial-number-verification-system-in-delphi/
// [2]: https://keygen.sh/blog/how-to-generate-license-keys-in-2021/

int PKV_GetKeyByte(int seed, int a, int b, int c)
{
    a = a % 25;
    b = b % 3;

    int result;
    if (a % 2 == 0)
        result = ((seed >> a) & 0x000000FF) ^ ((seed >> b) | c) & 0xff;
    else
        result = ((seed >> a) & 0x000000FF) ^ ((seed >> b) & c) & 0xff;

    return result;
}

string PKV_GetChecksum(string serial)
{
    var left = 0x0056; // 101
    var right = 0x00AF; // 175

    for (var i = 0; i < serial.Length; i++)
    {
        right += serial[i];
        if (right > 0x00ff)
            right -= 0x00ff;
    
        left += right;
        if (left > 0x00ff)
            left -= 0x00ff;
    }

    var sum = (left << 8) + right;

    return sum.ToString("X4");
}

string PKV_MakeKey(int seed)
{
    // build a list of subkeys
    // TODO: tweak the bit twiddling params and add more subkeys!
    var subkeys = new int[]
    {
        PKV_GetKeyByte(seed, 24, 3, 200),
        PKV_GetKeyByte(seed, 10, 0, 56),
        PKV_GetKeyByte(seed, 1, 2, 91),
        PKV_GetKeyByte(seed, 7, 1, 100),
    };
 
    // the key string begins with a hexidecimal string of the seed
    var result = seed.ToString("X8");

    // then is followed by hexidecimal strings of each byte in the key
    for (int i = 0; i < 4; i++)
    {
        result += subkeys[i].ToString("X2");
    }
 
    // add checksum to key string
    result += PKV_GetChecksum(result);

    // add some hyphens to make it easier to type
    var chunkSize = 4;
    result = string.Join('-', Enumerable.Range(0, result.Length / chunkSize)
                .Select(i => result.Substring(i * chunkSize, chunkSize)));
 
    return result;
}