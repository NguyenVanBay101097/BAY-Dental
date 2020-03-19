import { Component, OnInit } from '@angular/core';
import { FacebookPageMarketingCustomerConnectComponent } from '../facebook-page-marketing-customer-connect/facebook-page-marketing-customer-connect.component';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-facebook-page-marketing-customer-list',
  templateUrl: './facebook-page-marketing-customer-list.component.html',
  styleUrls: ['./facebook-page-marketing-customer-list.component.css']
})
export class FacebookPageMarketingCustomerListComponent implements OnInit {

  constructor(private modalService: NgbModal,) { }

  dataSendMessage: any [] = [];

  ngOnInit() {
  }

  showModalConnect() {
    let modalRef = this.modalService.open(FacebookPageMarketingCustomerConnectComponent, { size: 'lg', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    // modalRef.componentInstance.DataUser = this.DataUser;
    // modalRef.componentInstance.DataFanpages = this.DataFanpages;
    modalRef.result.then((result) => {
      if (result) {

        modalRef.close();
      }
    }, (reason) => {
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
