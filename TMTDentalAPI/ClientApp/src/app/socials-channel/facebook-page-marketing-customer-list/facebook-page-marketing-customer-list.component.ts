import { Component, OnInit } from '@angular/core';
import { FacebookPageMarketingCustomerConnectComponent } from '../facebook-page-marketing-customer-connect/facebook-page-marketing-customer-connect.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GridDataResult } from '@progress/kendo-angular-grid';
import { FacebookPageService } from '../facebook-page.service';
import { map } from 'rxjs/operators';
import { FacebookUserProfilesService } from '../facebook-user-profiles.service';

@Component({
  selector: 'app-facebook-page-marketing-customer-list',
  templateUrl: './facebook-page-marketing-customer-list.component.html',
  styleUrls: ['./facebook-page-marketing-customer-list.component.css']
})
export class FacebookPageMarketingCustomerListComponent implements OnInit {

  constructor(private modalService: NgbModal, 
    private facebookUserProfilesService: FacebookUserProfilesService) { }

  dataSendMessage: any [] = [];
  gridData: GridDataResult;
  limit = 20;
  skip = 0;
  loading = false;

  ngOnInit() {
    this.loadDataFromApi();
  }

  loadDataFromApi() {
    this.loading = true;
    var val = {
      limit: 10
    };
    this.facebookUserProfilesService.getPaged(val).pipe(
      map(response => (<GridDataResult>{
        data: response.items,
        total: response.totalItems
      }))
    ).subscribe(res => {
      this.gridData = res;
      this.loading = false;
      console.log(res);
    }, err => {
      console.log(err);
      this.loading = false;
    });
  }

  showModalConnectPartner(dataItem: any, rowIndex) {
    let modalRef = this.modalService.open(FacebookPageMarketingCustomerConnectComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.result.then((result) => {
      if (result) {
        var val = {
          "facebookUserId": dataItem.id,
          "partnerId": result
        }
        console.log(val);
        this.facebookUserProfilesService.connectPartner(val).subscribe(res => {
          this.loading = false;
          console.log(res);
          //this.loadDataFromApi();
          this.gridData.data[rowIndex].partnerId = result;
        }, err => {
          console.log(err);
          this.loading = false;
        });
        modalRef.close();
      }
    }, (reason) => {
    });
  }

  removePartner(dataItem: any, rowIndex) {
    var val = [
      dataItem.id
    ]
    this.facebookUserProfilesService.removePartner(val).subscribe(res => {
      this.loading = false;
      console.log(res);
      //this.loadDataFromApi();
      this.gridData.data[rowIndex].partnerId = null;
    }, err => {
      console.log(err);
      this.loading = false;
    });
  }

  onCreate(type, index) {
    var dataMessage;
    switch (type) {
      case 'text':
        dataMessage = {
          "type":"text",
          "text":""
        }
        break;
      case 'template_button':
        dataMessage = {
          "type":"template",
          "text":"",
          "type_template":"button",
          "buttons":[
            {
              "type":"web_url",
              "url":"https://www.messenger.com",
              "title":"Visit Messenger"
            }
          ]
        }
        break;
      case 'video':
        dataMessage = {
          "type":"video"
        }
        break;
      case 'image':
        dataMessage = {
          "type":"image"
        }
        break;
    }
    if (type === "template") {
      if (this.dataSendMessage[index].type === "text") {
        var temp_message = this.dataSendMessage[index].text;
        this.dataSendMessage[index] = dataMessage;
        this.dataSendMessage[index].text = temp_message;
      } else {
        if (this.dataSendMessage[index].type_template === "button") {
          this.dataSendMessage[index].buttons.push({

          });
        }
      }
    } else {
      dataMessage.type = type;
      this.dataSendMessage.push(dataMessage);
    }
    console.log(this.dataSendMessage);
  }

  onAdd(content, index) {

  }
}
