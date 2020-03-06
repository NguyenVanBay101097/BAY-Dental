import { Component, OnInit } from '@angular/core';
import { FacebookService, LoginResponse, LoginOptions } from 'ngx-facebook';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as $ from 'jquery';
import { FacebookDialogComponent } from '../facebook-dialog/facebook-dialog.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-facebook',
  templateUrl: './facebook.component.html',
  styleUrls: ['./facebook.component.css'],
  host: {
    class: 'o_action o_view_controller'
  }
})
export class FacebookComponent implements OnInit {
  accessToken: string;
  DataUser: any[] = [];
  DataFanpages: any[] = [];
  DataFanpagesConnected: any[] = [];
  dataCustomer: any[] = [];
  logged_in: boolean = false;
  selectedPage: any = null;
  selectedCustomer: any = null;

  constructor(private fb: FacebookService, 
    private notificationService: NotificationService,
    private modalService: NgbModal) { 
      fb.init({
        appId: '507339379926048',
        xfbml: true,
        version: 'v6.0'
      });
    }

  ngOnInit() {
  }

  login() {
    const loginOptions: LoginOptions = {
      enable_profile_selector: true,
      return_scopes: true,
      scope: 'public_profile,manage_pages,publish_pages,pages_messaging,read_page_mailboxes'
    };
    this.fb.login(loginOptions)
    .then((res: LoginResponse) => {
      console.log('Logged in', res);
      this.accessToken = res.authResponse.accessToken;
      this.getDataUser();
    })
    .catch(this.handleError);
  }

  logout() {
    this.fb.logout()
    .then((res) => {
      console.log('Logged out', res);
      this.accessToken = null;
      this.DataUser = [];
      this.DataFanpages  = [];
      this.DataFanpagesConnected = [];
      this.dataCustomer = [];
      this.logged_in = false;
      this.selectedPage = null;
      this.selectedCustomer = null;
    })
    .catch(this.handleError);
  }

  getDataUser() {
    this.fb.api('/me?fields=id,name,picture.type(large)')
    .then((res: any) => {
      console.log(res);
      this.DataUser.push({
        'id': res.id, 
        'name': res.name, 
        'picture': res.picture.data.url, 
        'accessToken': this.accessToken,
      });
      console.log('Got the data user', this.DataUser[0]);
      this.getDataFanpages();
    })
    .catch(this.handleError);
  }

  getDataFanpages() {
    this.fb.api('/me?fields=accounts{id,name,picture.type(large),access_token}')
    .then((res: any) => {
      console.log(res);
      var res_data = res.accounts.data;
      this.DataFanpages = [];
      for (var i = 0; i < res_data.length; i++) {
        this.DataFanpages.push({
          'id': res_data[i].id, 
          'name': res_data[i].name, 
          'picture': res_data[i].picture.data.url, 
          'accessToken': res_data[i].access_token,
        });
      }
      console.log('Got the data fanpages', this.DataFanpages);
      this.logged_in = true;
    })
    .catch(this.handleError);
  }

  getConversationsFanpage() {
    this.fb.api('/' + this.selectedPage.id +'?fields=conversations.limit(10){id,participants,snippet,updated_time}&access_token='+ this.selectedPage.accessToken)
    .then((res: any) => {
      console.log('Got the data fanpage', res);
      if (res.conversations) {
        var res_data = res.conversations.data;
        this.dataCustomer = [];
        for (var i = 0; i < res_data.length; i++) {
          this.dataCustomer.push({
            'id': res_data[i].participants.data[0].id, 
            'name': res_data[i].participants.data[0].name, 
            'picture': 'https://webelenz.com/wp-content/uploads/2019/11/testimonial.jpg',
            'phone': '090x.xxx.xxx',
            'snippet': res_data[i].snippet,
            'updated_time': res_data[i].updated_time,
            'editing_name': false,
            'editing_phone': false,
          });
        }
      }
      console.log('Data Conversations Fanpage', this.dataCustomer);
    })
    .catch(this.handleError);
  }

  getPictureCustomerDemo() {
    this.fb.api('/' + this.dataCustomer[0].id +'/picture?access_token='+ this.selectedPage.accessToken)
    .then((res: any) => {
      console.log('Got picture', res);
    })
    .catch(this.handleError);
  }

  private handleError(error) {
    console.error('Error processing action', error);
  }

  showModalAddConnect() {
    let modalRef = this.modalService.open(FacebookDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.DataUser = this.DataUser;
    modalRef.componentInstance.DataFanpages = this.DataFanpages;
    modalRef.result.then((result) => {
      if (result) {
        this.DataFanpagesConnected.push(result);
        modalRef.close();
      }
    }, (reason) => {
    });
  }

  selectPage(item) {
    this.selectedPage = item;
    this.getConversationsFanpage();
  }

  selectCustomer(item) {
    if (this.selectedCustomer === item) {
      this.selectedCustomer = null;
    } else {
      this.selectedCustomer = item;
    }
  }
}
