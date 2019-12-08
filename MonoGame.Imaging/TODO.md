
Redo/fix up `IImageDecoder` *again*:

Remove the `ImageReadStream` class (currently required by `IImageDecoder` and add a simple `Decode(Stream)` as a replacement. Move the `CancellationToken` disposal behavior from `ImageReadStream` into a new type: `CancellableStream` (which could then be put in `MonoGame.Utilities`).

`Decode(ImageReadStream)` currently returns a state object, which should be moved to a new function: `CreateState()` which should be then be used to (in the given order); 

1. `ReadHeader(State)`: Read the stream header, determining the format.
2. `ReadInfo(State)`: Read the image metadata and other information.
3. `ReadImage(State)`: Read the image.
4. `ReadNextImage(State)`: Read following image frames if the image has multiple frames/is animated.

*These method should either return `true`/`false` to indicate whether they succeeded or the original state object. Returning the state seems preferable, as it could itself have a `WasSuccessful` property.*

This remake should make `IImageDecoder` way more user friendly, as decoding should be more straightforward and progressive. Implementing new decoders should also be more streamlined. 

Extension methods for `IImageDecoder` could also be made, for example `Decode()` which could be a shortcut for getting to `ReadImage()` directly by calling `ReadHeader()` and `ReadInfo()` beforehand.

The best selling point of this will be the `ImageDecoderEnumerator`, both `IEnumerator` and `IEnumerable` so image frames can be read by a simple `foreach` loop.
*Implementation details*: `GetEnumerator()` returns `this`, breaking the `IEnumerable` pattern of being able to create multiple independent enumerators from one enumerable.
`Reset()` should throw `NotSupportedException` if the underlying stream can not seek.

The state object could be an interface which would give a bit more freedom. Decoders with special needs will of course be able to return special state objects that contain more than the base API exposes.

A problem that may happen with this API design is that there may be formats that can't be read in the specified order (Header -> Info -> Image), though it should really be rare occurrence.
That order does not cause trouble for any of the existing `stb` decoders.