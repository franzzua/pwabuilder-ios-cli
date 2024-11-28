typealias Extension = (Any) async -> String;

var extensions: [String: Extension] = [
    "signIn": signIn,
];
