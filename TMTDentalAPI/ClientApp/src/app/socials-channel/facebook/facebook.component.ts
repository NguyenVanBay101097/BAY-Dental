import { Component, OnInit } from '@angular/core';
import { FacebookService, LoginResponse, LoginOptions } from 'ngx-facebook';
import { NotificationService } from '@progress/kendo-angular-notification';
import * as $ from 'jquery';
import * as _ from 'lodash';
import { FacebookDialogComponent } from '../facebook-dialog/facebook-dialog.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { SocialsChannelDisplay, SocialsChannelService, SocialsChannelPaged, CheckPartner, MapPartner, PartnerMap } from '../socials-channel.service';
import { map } from 'rxjs/operators';
import { DatePipe } from '@angular/common';
import { PartnerPaged, PartnerSimple } from 'src/app/partners/partner-simple';
import { PartnerService } from 'src/app/partners/partner.service';
import { PartnerCreateUpdateComponent } from 'src/app/partners/partner-create-update/partner-create-update.component';
import { HttpParams } from '@angular/common/http';
import { FormGroup } from '@angular/forms';
import { PartnerCustomerCuDialogComponent } from 'src/app/partners/partner-customer-cu-dialog/partner-customer-cu-dialog.component';
import { FacebookPageService } from '../facebook-page.service';
import { FacebookConnectService } from '../facebook-connect.service';
import { FacebookConnectPageService } from '../facebook-connect-page.service';

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
  DataCustomers: any[] = [];
  DataMessagesCustomer: any[] = [];
  logged_in: boolean = false;
  selectedPage: any = null;
  selectedCustomer: any = null;
  //
  DataFanpagesDisplay: SocialsChannelDisplay = {
    'pageId': '',
    'accesstoken': ''
  };
  loading: boolean;
  limit = 10;
  skip = 0;
  search: string;
  //
  datePipe = new DatePipe('en-US');
  today = new Date();
  currentDate = this.today.toISOString();
  currentDateArray = this.convertDateToArray(this.currentDate);
  listPartners: Array<PartnerMap> = [];
  selectedPartner: any = null;
  searchNamePhone: string;
  linkedPartner: boolean = false;

  connect: any;

  constructor(private fb: FacebookService,
    private socialsChannelService: SocialsChannelService,
    private notificationService: NotificationService,
    private modalService: NgbModal,
    private partnerService: PartnerService,
    private facebookConnectService: FacebookConnectService,
    private facebookConnectPageService: FacebookConnectPageService) {
    fb.init({
      appId: '652339415596520',
      status: true,
      cookie: true,
      xfbml: true,
      version: 'v6.0'
    });
  }

  ngOnInit() {
    this.loadData();
    this.onGetPartnersList();
  }

  loadData() {
    this.facebookConnectService.get().subscribe((result: any) => {
      this.connect = result;
    });
  }

  login() {
    const loginOptions: LoginOptions = {
      enable_profile_selector: true,
      return_scopes: true,
      scope: 'public_profile,manage_pages,publish_pages,pages_messaging,read_page_mailboxes'
    };
    this.fb.login(loginOptions)
      .then((res: LoginResponse) => {
        // console.log('Logged in', res);
        // this.accessToken = res.authResponse.accessToken;
        // this.getDataUser();
        var val = {
          fbUserId: res.authResponse.userID,
          fbUserAccessToken: res.authResponse.accessToken
        };

        this.facebookConnectService.saveFromUI(val).subscribe(() => {
          this.loadData();
        });
      })
      .catch(this.handleError);
  }

  addConnect(item: any) {
    this.facebookConnectPageService.addConnect([item.id]).subscribe((result: any) => {
      this.loadData();
    });
  }

  logout() {
    this.fb.logout()
      .then((res) => {
        //console.log('Logged out', res);
        this.accessToken = null;
        this.DataUser = [];
        this.DataFanpages = [];
        this.DataFanpagesConnected = [];
        this.DataCustomers = [];
        this.DataMessagesCustomer = [];
        this.logged_in = false;
        this.selectedPage = null;
        this.selectedCustomer = null;
      })
      .catch(this.handleError);
  }

  getDataUser() {
    this.fb.api('/me?fields=id,name,picture.type(large)')
      .then((res: any) => {
        //console.log(res);
        this.DataUser.push({
          'id': res.id,
          'name': res.name,
          'picture': res.picture.data.url,
          'accessToken': this.accessToken,
        });
        //console.log('Got the data user', this.DataUser[0]);
        this.getDataFanpages();
      })
      .catch(this.handleError);
  }

  getDataFanpages() {
    this.fb.api('/me?fields=accounts{id,name,picture.type(large),access_token}')
      .then((res: any) => {
        //console.log(res);
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
        //console.log('Got the data fanpages', this.DataFanpages);
        this.onGetFanpagesFromDB();
        this.logged_in = true;
      })
      .catch(this.handleError);
  }

  getConversationsFanpage() {
    var tempParam = 'id,participants,snippet,unread_count,messages.limit(1){from},updated_time';
    this.fb.api('/' + this.selectedPage.id + '?fields=conversations.limit(10){' + tempParam + '}&access_token=' + this.selectedPage.accessToken)
      .then((res: any) => {
        //console.log('Got the data fanpage', res);
        if (res.conversations) {
          var res_data = res.conversations.data;
          this.DataCustomers = [];
          var message_from_page = false;
          for (var i = 0; i < res_data.length; i++) {
            message_from_page = false;
            if (res_data[i].messages.data[0].from.id === this.selectedPage.id) {
              message_from_page = true;
            }
            this.DataCustomers.push({
              'id': res_data[i].participants.data[0].id,
              'name': res_data[i].participants.data[0].name,
              'picture': 'https://webelenz.com/wp-content/uploads/2019/11/testimonial.jpg',
              'phone': '090x.xxx.xxx',
              'snippet': res_data[i].snippet,
              'unread_count': res_data[i].unread_count,
              'message_from_page': message_from_page,
              'updated_time': this.formatDateForListCustomer(res_data[i].updated_time),
              'conversation_id': res_data[i].id,
              'editing_name': false,
              'editing_phone': false,
            });
          }
        }
        //console.log('Data Conversations Fanpage', this.DataCustomers);
      })
      .catch(this.handleError);
  }

  getMessagesFanpageWithID(conversation_id) {
    this.fb.api('/' + conversation_id + '?fields=messages{message,from,created_time}&access_token=' + this.selectedPage.accessToken)
      .then((res: any) => {
        //console.log('Got the data messages with id ', res);
        if (res.messages) {
          var res_data = res.messages.data;
          this.DataMessagesCustomer = [];
          var message_from_page = false;
          for (var i = 0; i < res_data.length; i++) {
            message_from_page = false;
            if (res_data[i].from.id === this.selectedPage.id) {
              message_from_page = true;
            }
            this.DataMessagesCustomer.push({
              'id': res_data[i].id,
              'message': res_data[i].message,
              'message_from_page': message_from_page,
              'created_time': this.formatDateForMessagesCustomer(res_data[i].created_time),
            });
          }
          //console.log('DataMessagesCustomer ', this.DataMessagesCustomer);
          this.DataMessagesCustomer = this.groupDataMessagesCustomer(this.DataMessagesCustomer.slice().reverse());
        }
        //console.log('Data MessagesFanpageWithID ', this.DataMessagesCustomer);
        $('#message_box').animate({ scrollTop: 9999 });
      })
      .catch(this.handleError);
  }

  getPictureCustomerDemo() {
    this.fb.api('/' + this.DataCustomers[0].id + '/picture?access_token=' + this.selectedPage.accessToken)
      .then((res: any) => {
        console.log('Got picture', res);
      })
      .catch(this.handleError);
  }

  handleError(error) {
    console.error('Error processing action', error);
  }

  showModalAddConnect() {
    let modalRef = this.modalService.open(FacebookDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.DataUser = this.DataUser;
    modalRef.componentInstance.DataFanpages = this.DataFanpages;
    modalRef.result.then((result) => {
      if (result) {
        this.onAddFanpageToDB(result.id, this.accessToken);
        this.onGetFanpagesFromDB();
        modalRef.close();
      }
    }, (reason) => {
    });
  }

  showModalCreatePartner() {
    let modalRef = this.modalService.open(PartnerCustomerCuDialogComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.title = 'Thêm khách hàng';

    modalRef.result.then(res => {
      this.onGetPartnersList();
      this.selectedPartner = {
        'id': null,
        'partnerId': res.id,
        'partnerName': res.name,
        'partnerPhone': res.phone,
        'partnerEmail': null
      };
      this.linkedPartner = true;
    }, () => {
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
      this.getMessagesFanpageWithID(item.conversation_id);
      this.onCheckPartner();
    }
  }

  formatDate(date: string) {
    return this.datePipe.transform(date, "EEE, d/M/y, w, W, HH:mm, z");
  }
  convertWeekDay(weekDay) {
    switch (weekDay) {
      case 'Mon':
        return 'T2';
      case 'Tue':
        return 'T3';
      case 'Wed':
        return 'T4';
      case 'Thu':
        return 'T5';
      case 'Fri':
        return 'T6';
      case 'Sat':
        return 'T7';
      case 'Sun':
        return 'CN';
    }
  }
  convertDateToArray(date: string) {
    var dateFormatted = this.formatDate(date);
    var dateSplitted = dateFormatted.split(", ");
    return {
      'weekDay': this.convertWeekDay(dateSplitted[0]),
      'day': dateSplitted[1].split('/')[0],
      'month': dateSplitted[1].split('/')[1],
      'year': dateSplitted[1].split('/')[2],
      'weekOfYear': dateSplitted[2],
      'weekOfMonth': dateSplitted[3],
      'hour': dateSplitted[4].split(':')[0],
      'minute': dateSplitted[4].split(':')[1],
      'zone': dateSplitted[5],
    };
  }
  formatDateForListCustomer(date: string) {
    var dateArray = this.convertDateToArray(date);
    if (dateArray.year === this.currentDateArray.year) {
      if (dateArray.weekOfYear === this.currentDateArray.weekOfYear) {
        if (dateArray.day === this.currentDateArray.day) {
          return dateArray.hour + ':' + dateArray.minute; //00:00
        } else {
          return dateArray.weekDay; //T0
        }
      } else {
        return dateArray.day + ' Tháng ' + dateArray.month; //00 Tháng 00
      }
    } else {
      return dateArray.day + '/' + dateArray.month + '/' + dateArray.year; //00/00/0000
    }
  }

  formatDateForMessagesCustomer(date: string) {
    var dateArray = this.convertDateToArray(date);
    if (dateArray.year === this.currentDateArray.year) {
      if (dateArray.weekOfYear === this.currentDateArray.weekOfYear) {
        if (dateArray.day === this.currentDateArray.day) {
          return dateArray.hour + ':' + dateArray.minute; //00:00
        } else {
          return dateArray.weekDay + ' ' + dateArray.hour + ':' + dateArray.minute; //T0 00:00
        }
      } else {
        return dateArray.hour + ':' + dateArray.minute + ', ' +
          dateArray.day + ' THÁNG ' + dateArray.month + ', ' + dateArray.year; //00:00, 00 THÁNG 00, 000
      }
    } else {
      return dateArray.hour + ':' + dateArray.minute + ', ' +
        dateArray.day + ' THÁNG ' + dateArray.month + ', ' + dateArray.year; //00:00, 00 THÁNG 00, 000
    }
  }

  groupDataMessagesCustomer(dataMessagesCustomer) {
    var result = [];
    var temp = [];
    var created_time = '';
    for (var i = 0; i < dataMessagesCustomer.length; i++) {
      if (dataMessagesCustomer[i].created_time !== created_time) {
        if (temp.length) {
          result.push(temp);
          temp = [];
        }
        created_time = dataMessagesCustomer[i].created_time;
      }
      if (dataMessagesCustomer[i - 1]) {
        if (dataMessagesCustomer[i - 1].message_from_page === false &&
          dataMessagesCustomer[i].message_from_page === true) {
          dataMessagesCustomer[i - 1]['last_message'] = true;
          dataMessagesCustomer[i]['first_message'] = true;
        }
        if (dataMessagesCustomer[i - 1].message_from_page === true &&
          dataMessagesCustomer[i].message_from_page === false) {
          dataMessagesCustomer[i - 1]['last_message'] = true;
          dataMessagesCustomer[i]['first_message'] = true;
        }
      }
      temp.push(dataMessagesCustomer[i]);
    }
    if (temp.length) {
      result.push(temp);
      temp = [];
    }
    for (var i = 0; i < result.length; i++) {
      var length_result_i = result[i].length;
      result[i][length_result_i - 1]['last_message'] = true;
      result[i][0]['first_message'] = true;
    }
    return result;
  }

  onGetPartnersList() {
    this.loading = true;
    var val = new PartnerPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.searchNamePhone || '';
    val.customer = true;
    val.supplier = false;
    this.partnerService.getPartnerPaged(val).pipe(
      map(response => (<any>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      console.log('onGetPartnersList', res);
      var res_data = res.data;
      this.listPartners = [];
      for (var i = 0; i < res_data.length; i++) {
        this.listPartners.push({
          'id': null,
          'partnerId': res_data[i].id,
          'partnerName': res_data[i].name,
          'partnerPhone': res_data[i].phone,
          'partnerEmail': null
        });
      }
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = true;
    }
    )
  }

  onGetFanpagesFromDB() {
    this.loading = true;
    var val = new SocialsChannelPaged();
    val.limit = this.limit;
    val.offset = this.skip;
    val.search = this.search || '';
    this.socialsChannelService.getPageBasic(val).pipe(
      map(response => (<any>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      //console.log('onGetFanpagesFromDB', res);
      var res_data = res.data;
      this.DataFanpagesConnected = [];
      for (var i = 0; i < this.DataFanpages.length; i++) {
        for (var j = 0; j < res_data.length; j++) {
          if (this.DataFanpages[i].id === res_data[j].pageId) {
            this.DataFanpagesConnected.push(this.DataFanpages[i]);
          }
        }
      }
      //console.log('DataFanpagesConnected', this.DataFanpagesConnected);
      this.loading = false;
    }, err => {
      console.log(err);
      this.loading = false;
    })
  }

  onGetFanpageWithIDFromDB(fanpage_id) {
    this.loading = true;
    this.socialsChannelService.get(fanpage_id).subscribe(
      res => {
        //console.log('onGetFanpageWithIDFromDB', res);
        this.loading = false;
      }, err => {
        console.log(err);
        this.loading = false;
      })
  }

  onAddFanpageToDB(fanpage_id, fanpage_accessToken) {
    this.loading = true;
    this.DataFanpagesDisplay.pageId = fanpage_id;
    this.DataFanpagesDisplay.accesstoken = fanpage_accessToken;
    this.socialsChannelService.create(this.DataFanpagesDisplay).subscribe(
      res => {
        //console.log(res);
        this.loading = false;
      },
      err => {
        console.log(err);
        this.loading = true;
      }
    );
  }

  onCheckPartner() {
    this.loading = true;
    var val = new CheckPartner();
    val.PageId = this.selectedPage.id;
    val.PSId = this.selectedCustomer.id;
    this.socialsChannelService.checkPartner(val).subscribe(
      res => {
        //console.log(res);
        this.onGetPartnersList();
        if (res.partnerName) {
          this.selectedPartner = {
            'id': res.id,
            'partnerId': res.partnerId,
            'partnerName': res.partnerName,
            'partnerPhone': res.partnerPhone,
            'partnerEmail': res.partnerEmail
          };
          this.linkedPartner = true;
        } else {
          this.selectedPartner = null;
          this.linkedPartner = false;
        }
        console.log('onCheckPartner', this.selectedPartner);
        this.loading = false;
      },
      err => {
        console.log(err);
        this.loading = true;
      }
    );
  }

  onMapPartner() {
    this.loading = true;
    var val = new MapPartner();
    val.PartnerId = this.selectedPartner.partnerId;
    val.PageId = this.selectedPage.id;
    val.PSId = this.selectedCustomer.id;
    console.log('onMapPartner', this.selectedPartner);
    this.socialsChannelService.mapPartner(val).subscribe(
      res => {
        //console.log(res);
        this.selectedPartner = {
          'id': res.id,
          'partnerId': res.partnerId,
          'partnerName': res.partnerName,
          'partnerPhone': res.partnerPhone,
          'partnerEmail': res.partnerEmail
        };
        this.linkedPartner = true;
        this.loading = false;
      },
      err => {
        console.log(err);
        this.loading = true;
      }
    );
  }

  onUnlinkPartner() {
    this.loading = true;
    console.log('onUnlinkPartner', this.selectedPartner);
    this.socialsChannelService.unlinkPartner([this.selectedPartner.id]).subscribe(
      res => {
        console.log(res);
        this.selectedPartner = null;
        this.linkedPartner = false;
        this.loading = false;
      },
      err => {
        console.log(err);
        this.loading = true;
      }
    );
  }
}
