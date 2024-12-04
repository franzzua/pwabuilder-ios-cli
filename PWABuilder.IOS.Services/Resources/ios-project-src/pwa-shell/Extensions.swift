import AuthenticationServices
import GoogleSignIn

typealias Extension = (String, ViewController,
                       @escaping (String?, String?) -> ()) -> ();

var extensions: [String: Extension] = [
    "signIn": signIn,
];

enum LoginError: Error {
    case reject(String)
}
extension String: Error {}

func signIn(_ provider: String, ctrl: ViewController, completion: @escaping (String?, String?) -> ()) -> () {
    switch provider{
    case "apple":
        authApple(ctrl: ctrl, completion: completion)
        break
    case "google":
        authGoogle(ctrl: ctrl, completion: completion)
        break
    default:
        completion("", "Sign in with \(provider) is not implemented")
        break
    }
}

func authGoogle(ctrl: ViewController, completion: @escaping (String?, String?) -> ()) -> () {
    GIDSignIn.sharedInstance.signIn(withPresenting: ctrl) { (result, error) in
        if error != nil {
            completion("", error?.localizedDescription)
            return
        }
        completion(result?.user.idToken?.tokenString, nil)
    }
}

func authApple(ctrl: ViewController, completion: @escaping (String?, String?) -> ()) -> () {
    let authorizationProvider = ASAuthorizationAppleIDProvider()
    let request = authorizationProvider.createRequest()
    request.requestedScopes = [.email, .fullName]
    let authorizationController = ASAuthorizationController(authorizationRequests: [request])
    authorizationController.delegate = ctrl
    authorizationController.presentationContextProvider = ctrl
    authorizationController.performRequests()
    func handler(token: String?, error: String?) {
        completion(token, error)
    }
    complitionHandler = handler
}

var complitionHandler: ((String?, String?)->())?

@available(iOS 13.0, *)
extension ViewController: ASAuthorizationControllerDelegate {


    func authorizationController(controller: ASAuthorizationController, didCompleteWithAuthorization authorization: ASAuthorization) {
        guard let appleIDCredential = authorization.credential as? ASAuthorizationAppleIDCredential else {
            return
        }

        complitionHandler?(String(data: appleIDCredential.identityToken!, encoding: .utf8), nil)

    }

    func authorizationController(controller: ASAuthorizationController, didCompleteWithError error: Error) {
        complitionHandler?(nil, error.localizedDescription)
        print("AppleID Credential failed with error: \(error.localizedDescription)")
    }
}

@available(iOS 13.0, *)
extension ViewController: ASAuthorizationControllerPresentationContextProviding {
    func presentationAnchor(for controller: ASAuthorizationController) -> ASPresentationAnchor {
        return self.webviewView.window!
    }
}
