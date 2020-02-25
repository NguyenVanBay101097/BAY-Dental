import { Component, OnInit } from '@angular/core';
declare var FB: any;

@Component({
  selector: 'app-facebook-config-establish',
  templateUrl: './facebook-config-establish.component.html',
  styleUrls: ['./facebook-config-establish.component.css']
})
export class FacebookConfigEstablishComponent implements OnInit {
  applicationId = '652339415596520';
  showAuthUser = false;
  isValidCredentials = false;
  userName: string;
  userEmail: string;
  pageaccountDetails: any;
  constructor() { }

  ngOnInit() {
    this.loadFBSDK();
  }

  loadFBSDK() {
    let self = this;
    (<any>window).fbAsyncInit = () => {
      FB.init({
        appId: self.applicationId,
        xfbml: false,
        version: 'v6.0'
      });
    };

    (function (d, s, id) {
      let js, fjs = d.getElementsByTagName(s)[0];
      if (d.getElementById(id)) { return; }
      js = d.createElement(s); js.id = id;
      js.src = "//connect.facebook.net/en_US/sdk.js";
      fjs.parentNode.insertBefore(js, fjs);
    }(document, 'script', 'facebook-js-sdk'));
  }

  submitLogin() {
    FB.login((loginResponse) => {
      localStorage.setItem("fb_access_token", loginResponse.authResponse.accessToken);
      this.showAuthUser = true;
      this.getUserInfo(loginResponse.authResponse.userID);
      this.isValidCredentials = true;
    }, { scope: "public_profile,email,manage_pages,pages_messaging,pages_messaging_phone_number" });
  }

  getUserInfo(userId) {
    console.log("UserId" + userId);
    let self = this;
    FB.api(
      "/" + userId + '?fields=name,accounts,email',
      (result) => {
        if (result && !result.error) {
          console.log('result: ', result);
          self.userName = result.name;
          self.userEmail = result.email;
          self.pageaccountDetails = result.accounts.data
        }
        else {
          this.isValidCredentials = false;
        }
      });
  }

}
