import { Component, AfterViewInit, HostListener, ElementRef } from '@angular/core';
import { AuthService } from './auth/auth.service';
import { Router } from '@angular/router';
import { PrintService } from './print.service';
declare var $: any;
import * as _ from 'lodash';
import { PermissionService } from './shared/permission.service';
import { ImportSampleDataComponent } from './shared/import-sample-data/import-sample-data.component';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { IrConfigParameterService } from './core/services/ir-config-parameter.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'tmt-dental';
  _areAccessKeyVisible = false;
  value: string;
  constructor(
    public authService: AuthService,
    private router: Router,
    public printService: PrintService,
    private notificationService: NotificationService,
    private el: ElementRef,
    private permissionService: PermissionService,
    private http: HttpClient,
    private modalService: NgbModal,
    private irConfigParamService: IrConfigParameterService) {
    this.loadGroups();

    this.authService.currentUser.subscribe((user) => {
      if (user) {
        this.loadIrConfigParam();
        this.authService.getGroups().subscribe((result: any) => {
          console.log(result);
          
          this.permissionService.define(result);
        });
      }
    });
    
    if (this.authService.isAuthenticated()) {
      this.loadIrConfigParam();
      this.authService.getGroups().subscribe((result: any) => {
        this.permissionService.define(result);
      });
    }
  }
  @HostListener('document:keydown', ['$event']) onKeydownHandler(keyDownEvent: KeyboardEvent) {
    if (!this._areAccessKeyVisible &&
      (keyDownEvent.altKey || keyDownEvent.key === 'Alt') &&
      !keyDownEvent.ctrlKey) {

      this._areAccessKeyVisible = true;
      this._addAccessKeyOverlays();
    }
  }

  @HostListener('document:keyup', ['$event']) onKeyupHandler(keyUpEvent: KeyboardEvent) {
    if ((keyUpEvent.altKey || keyUpEvent.key === 'Alt' || keyUpEvent.key === 'Tab') && !keyUpEvent.ctrlKey) {
      this._hideAccessKeyOverlay();
      if (keyUpEvent.preventDefault) keyUpEvent.preventDefault(); else keyUpEvent.returnValue = false;
      if (keyUpEvent.stopPropagation) keyUpEvent.stopPropagation();
      if (keyUpEvent.cancelBubble) keyUpEvent.cancelBubble = true;
      return false;
    }
  }

  _hideAccessKeyOverlay() {
    this._areAccessKeyVisible = false;
    var overlays = $(document).find('.o_web_accesskey_overlay');
    if (overlays.length) {
      return overlays.remove();
    }
  }

  _addAccessKeyOverlays() {
    var accesskeyElements = $(document).find('[accesskey]').filter(':visible');
    _.each(accesskeyElements, function (elem) {
      var overlay = $("<div class='o_web_accesskey_overlay'>" + $(elem).attr('accesskey').toUpperCase() + "</div>");
      var $overlayParent;
      if (elem.tagName.toUpperCase() === "INPUT") {
        // special case for the search input that has an access key
        // defined. We cannot set the overlay on the input itself,
        // only on its parent.
        $overlayParent = $(elem).parent();
      } else {
        $overlayParent = $(elem);
      }

      if ($overlayParent.css('position') !== 'absolute') {
        $overlayParent.css('position', 'relative');
      }
      overlay.appendTo($overlayParent);
    });
  }

  loadGroups() {
    if (this.authService.isAuthenticated()) {
      this.authService.currentUser.subscribe(user => {
        if (user) {
          this.authService.getGroups().subscribe((result: any) => {
            this.permissionService.define(result);
          });
        }
      });
    }
  }

  loadIrConfigParam() {
    var key = "import_sample_data";
    this.irConfigParamService.getParam(key).subscribe(
      (result: any) => {
        this.value = result.value;
        if (!this.value) {
          this.openPopupImportSimpleData();
        }
      }
    )
  }

  openPopupImportSimpleData() {
    const modalRef = this.modalService.open(ImportSampleDataComponent, { scrollable: true, size: 'sm', windowClass: 'o_technical_modal', keyboard: false, backdrop: 'static' });
    modalRef.componentInstance.value = this.value;
    modalRef.result.then(result => {
      if (result) {
        this.notificationService.show({
          content: 'Khởi tạo dữ liệu mẫu thành công',
          hideAfter: 3000,
          position: { horizontal: 'center', vertical: 'top' },
          animation: { type: 'fade', duration: 400 },
          type: { style: 'success', icon: true }
        });
        window.location.reload();
      }
    }, err => {
      console.log(err);
    });
  }

}
