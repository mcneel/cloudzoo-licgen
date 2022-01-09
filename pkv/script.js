const crypto = require('crypto')
 
// Format a number to a fixed-length hexidecimal string
function toFixedHex(num, len) {
  return num.toString(16).toUpperCase().padStart(len, '0').substring(0, len)
}
 
// Derive a subkey from the seed (a, b, c being params for bit twiddling)
function getSubkeyFromSeed(seed, a, b, c) {
  if (typeof seed === 'string') {
    seed = parseInt(seed, 16)
  }

//   console.log(seed)
//   console.log(`a: ${a}, b: ${b}, c: ${c}`)
 
  a = a % 25
  b = b % 3

//   console.log(`a: ${a}, b: ${b}`)
//   console.log(a % 2)
 
  let subkey
  if (a % 2 === 0) {
    subkey = ((seed >> a) & 0x000000ff) ^ ((seed >> b) | c) & 0xff
  } else {
    subkey = ((seed >> a) & 0x000000ff) ^ ((seed >> b) & c) & 0xff
  }

//   console.log(`${subkey} -> ${toFixedHex(subkey, 2)}`)
//   console.log()
 
  return toFixedHex(subkey, 2)
}
 
// Get the checksum for a given serial string
function getChecksumForSerial(serial) {
    // console.log(serial)
  let right = 0x00af // 175
  let left = 0x0056 // 101
 
  for (var i = 0; i < serial.length; i++) {
    right += serial.charCodeAt(i)
    if (right > 0x00ff) {
      right -= 0x00ff
    }
 
    left += right
    if (left > 0x00ff) {
      left -= 0x00ff
    }
  }

//   console.log(`${left}, ${right}`)
 
  const result = (left << 8) + right
  
//   console.log(result)
//   console.log(toFixedHex(result, 4))

  return toFixedHex(result, 4)
}
 
// Format the key (XXXX-XXXX-XXXX-XXXX-XXXX)
function formatKey(key) {
  return key.match(/.{4}/g).join('-')
}
 
// Generate a 4-byte hexidecimal seed value
function generateSeed(n) {
  const seed = crypto.randomBytes(4).toString('hex')

//   console.log(seed)
 
  return seed.toUpperCase()
}
 
// Generate a (legitimate) license key
function generateKey(seed) {
  // Build a list of subkeys (bit twiddling params are arbitrary but can never change)
  const subkeys = [
    getSubkeyFromSeed(seed, 24, 3, 200),
    getSubkeyFromSeed(seed, 10, 0, 56),
    getSubkeyFromSeed(seed, 1, 2, 91),
    getSubkeyFromSeed(seed, 7, 1, 100),
  ]
 
  // Build the serial (seed + subkeys)
  const serial = seed + subkeys.join('')
 
  // Build the key (serial + checksum)
  const key = serial + getChecksumForSerial(serial)
 
  return formatKey(key)
}

// const seed = generateSeed()
// console.log(seed)
// const key = generateKey(seed)

// const seed = '396C2294'
let hash = 1044894456

// https://stackoverflow.com/a/697841
if (hash < 0)
  hash = 0xFFFFFFFF + hash + 1;

const seed = hash.toString(16).toUpperCase()
const key = generateKey(seed)
 
console.log(key)