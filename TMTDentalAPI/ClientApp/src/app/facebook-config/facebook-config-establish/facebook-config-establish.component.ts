import { Component, OnInit } from '@angular/core';
import { FacebookConfigSave } from '../shared/facebook-config-save';
import { FacebookConfigService } from '../shared/facebook-config.service';
import { Router } from '@angular/router';
import { FacebookOAuthService } from '../shared/facebook-oauth.service';
import { Subject } from 'rxjs';
import { FacebookConfigConnectSave } from '../shared/facebook-config-connect-save';
declare var FB: any;

@Component({
  selector: 'app-facebook-config-establish',
  templateUrl: './facebook-config-establish.component.html',
  styleUrls: ['./facebook-config-establish.component.css']
})
export class FacebookConfigEstablishComponent implements OnInit {
  // applicationId = '652339415596520';
  applicationId = '327268081110321';
  showAuthUser = false;
  isValidCredentials = false;
  userName: string;
  userEmail: string;
  pageaccountDetails: any;

  mySubject = new Subject<any>();

  constructor(private facebookConfigService: FacebookConfigService, private router: Router,
    private facebookOAuthService: FacebookOAuthService) { }

  ngOnInit() {
    if (localStorage.getItem('facebook_config_id')) {
      this.router.navigate(['/facebook-config/setting']);
    } else {
      this.loadFBSDK();
    }


  }

  onSave() {
    var accessToken = localStorage.getItem('fb_access_token');
    var userId = localStorage.getItem('fb_user_id');
    if (!accessToken || !userId) {
      alert('you must login facebook first');
      return false;
    }

    var val = new FacebookConfigConnectSave();
    val.userId = userId;
    val.accessToken = accessToken;

    this.facebookConfigService.connect(val).subscribe((config) => {
      this.router.navigate(['/facebook-config/' + config.id + '/setting']);
    });
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

  isAuthenticated() {
    var res = localStorage.getItem('fb_access_token') !== null;
    return res;
  }

  logout() {
    this.showAuthUser = false;
    localStorage.removeItem('fb_access_token');
  }

  submitLogin() {
    FB.login((loginResponse) => {
      if (loginResponse.status === 'connected') {
        localStorage.setItem("fb_access_token", loginResponse.authResponse.accessToken);
        localStorage.setItem("fb_user_id", loginResponse.authResponse.userID);
      } else {
      }
    }, { scope: "public_profile,pages_show_list,manage_pages,pages_messaging,read_page_mailboxes" });
  }

  checkLoginState() {               // Called when a person is finished with the Login Button.
    let self = this;
    FB.getLoginStatus(function (response) {   // See the onlogin handler
      self.statusChangeCallback(response);
    });
  }

  statusChangeCallback(response) {  // Called with the results from FB.getLoginStatus().
    console.log('statusChangeCallback');
    console.log(response);                   // The current login status of the person.
    if (response.status === 'connected') {   // Logged into your webpage and Facebook.
    } else {                                 // Not logged into your webpage or we are unable to tell.
    }
  }

  getUserInfo(userId, accessToken) {
    let self = this;
    FB.api(
      "/" + userId + '?fields=name,accounts',
      (result) => {
        console.log('result: ', result);
        if (result && !result.error) {
          var saveData = new FacebookConfigSave();
          saveData.fbAccountUserId = userId;
          saveData.fbAccountName = result.name;
          saveData.userAccessToken = accessToken;
          saveData.configPages = result.accounts.data.map(x => {
            return {
              pageId: x.id,
              pageName: x.name,
              pageAccessToken: x.access_token
            };
          });
          console.log('saveData: ', saveData);
          this.facebookConfigService.create(saveData).subscribe(config => {
            localStorage.setItem('facebook_config_id', config.id);
            this.router.navigate(['/facebook-config/setting']);
          });
        }
        else {
          this.isValidCredentials = false;
        }
      });
  }

}
