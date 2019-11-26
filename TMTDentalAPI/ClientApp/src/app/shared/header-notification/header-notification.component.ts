import { Component, OnInit } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { environment } from '../../../environments/environment';
import { AuthService } from 'src/app/auth/auth.service';
import { MailMessageFormat } from 'src/app/core/mail-message-format';
import { MailMessageService, MailMessageFetch } from 'src/app/mail-messages/mail-message.service';
import { Howl } from 'howler';
import { NotificationService } from '@progress/kendo-angular-notification';
import { AppointmentService } from 'src/app/appointment/appointment.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AppointmentDetailDialogComponent } from 'src/app/appointment/appointment-detail-dialog/appointment-detail-dialog.component';


@Component({
  selector: 'app-header-notification',
  templateUrl: './header-notification.component.html',
  styleUrls: ['./header-notification.component.css']
})
export class HeaderNotificationComponent implements OnInit {
  messages: MailMessageFormat[] = [];
  messageCount = 0;
  constructor(private authService: AuthService, private mailMessageService: MailMessageService,
    private notificationService: NotificationService, private appointmentService: AppointmentService,
    private modalService: NgbModal) { }
  myDate = new Date();
  ngOnInit() {

    const connection = new signalR.HubConnectionBuilder()
      .configureLogging(signalR.LogLevel.Information)
      .withUrl(environment.apiDomain + "notification", {
        accessTokenFactory: () => this.authService.getAuthorizationToken(),
        skipNegotiation: true,
        transport: signalR.HttpTransportType.WebSockets
      })
      .build();

    connection.start().then(function () {
      console.log('Connected');
    }).catch(function (err) {
      console.error(err.toString());
    });

    connection.on("broadcastMessage", (message: MailMessageFormat) => {
      this.messages.unshift(message);
      this.messageCount = this.messageCount + 1;
      this.notificationService.show({
        content: 'Bạn có 1 thông báo mới',
        hideAfter: 3000,
        position: { horizontal: 'right', vertical: 'top' },
        animation: { type: 'fade', duration: 400 },
        type: { style: 'success', icon: true }
      });
      this.soundNotify();
    });

    this.loadMessages();
  }

  resetMessageCount() {
    this.messageCount = 0;
  }

  loadMessages() {
    var val = new MailMessageFetch();
    val.limit = 10;
    this.mailMessageService.messageFetch(val).subscribe(result => {
      this.messages = result;
    });
  }

  soundNotify() {
    var sound = new Howl({
      src: ['./assets/sounds/jingle-bells-sms.mp3']
    });
    sound.play();
  }

  notificationClick(message: MailMessageFormat) {
    if (message.model && message.resId) {
      if (message.model == 'appointment') {
        this.appointmentService.getBasic(message.resId).subscribe(ap => {
          const modalRef = this.modalService.open(AppointmentDetailDialogComponent, { size: 'xl', windowClass: 'o_technical_modal' });
          modalRef.componentInstance.appointment = ap;
          modalRef.result.then(() => {
          });
        }, () => {
          this.notificationService.show({
            content: 'Không tìm thấy lịch hẹn, có thể lịch hẹn đã bị xóa!',
            hideAfter: 3000,
            position: { horizontal: 'right', vertical: 'top' },
            animation: { type: 'fade', duration: 400 },
            type: { style: 'error', icon: true }
          });
        });
      }
    }
  }
}
