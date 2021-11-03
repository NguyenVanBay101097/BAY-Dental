import { Component, AfterViewInit, HostListener, ElementRef, OnInit } from '@angular/core';
import { AuthService } from './auth/auth.service';
import { Router } from '@angular/router';
declare var $: any;
import * as _ from 'lodash';
import { PermissionService } from './shared/permission.service';
import { ImportSampleDataComponent } from './shared/import-sample-data/import-sample-data.component';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpParams } from '@angular/common/http';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { NotificationService } from '@progress/kendo-angular-notification';
import { IrConfigParameterService } from './core/services/ir-config-parameter.service';
import { SwUpdate } from '@angular/service-worker';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'TDental';
  _areAccessKeyVisible = false;
  constructor(
    public authService: AuthService,
    private router: Router,
    private notificationService: NotificationService,
    private el: ElementRef,
    private permissionService: PermissionService,
    private http: HttpClient,
    private modalService: NgbModal,
    private swUpdate: SwUpdate) {
    this.loadGroups();

    this.authService.currentUser.subscribe((user) => {
      if (user) {
        this.authService.getGroups().subscribe((result: any) => {
          this.permissionService.define(result);
        });
      }
    });
    if (this.authService.isAuthenticated()) {
      this.authService.getGroups().subscribe((result: any) => {
        this.permissionService.define(result);
      });
    }
  }

  ngOnInit(): void {
    console.log('swUpdate ' + this.swUpdate.isEnabled);
    if (this.swUpdate.isEnabled) {
      this.swUpdate.available.subscribe(() => {
        if (confirm("Có phiên bản mới, tải ngay?")) {
          window.location.reload();
        }
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
      this.authService.getGroups().subscribe((result: any) => {
        this.permissionService.define(result);
      });
      // this.authService.currentUser.subscribe(user => {
      //   if (user) {
      //     this.authService.getGroups().subscribe((result: any) => {
      //       console.log(result);
      //       this.permissionService.define(result);
      //     });
      //   }
      // });
    }
  }

}
