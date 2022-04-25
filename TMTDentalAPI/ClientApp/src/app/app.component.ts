import { Component, HostListener, OnInit } from '@angular/core';
import { SwUpdate } from '@angular/service-worker';
import * as _ from 'lodash';
import { AuthService } from './auth/auth.service';
declare var $: any;
import { environment } from '../environments/environment';
import { Router } from '@angular/router';
import { WebSessionService } from './core/services/web-session.service';

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
    private swUpdate: SwUpdate,
    private router: Router,
    private webSessionService: WebSessionService) {

    if (this.swUpdate.isEnabled) {
      this.swUpdate.available.subscribe(() => {
        if (confirm("Có cập nhật mới, tải ngay?")) {
          window.location.reload();
        }
      });
    }

    if (this.authService.isAuthenticated()) {
      this.webSessionService.getSessionInfo().subscribe();
    }
    
    this.authService.currentUser.subscribe((user) => {
      if (user) {
        this.webSessionService.getSessionInfo().subscribe();
      }
    });
  }

  ngOnInit(): void {
    this.checkAppVersion();
  }

  checkAppVersion() {
    var version = localStorage['app_version'] ? parseInt(localStorage['app_version']) : 0;
    if (version != environment.version) {
      this.authService.logout();
      this.router.navigate(['/auth/login']);
    }
  }

  // @HostListener('document:keydown', ['$event']) onKeydownHandler(keyDownEvent: KeyboardEvent) {
  //   if (!this._areAccessKeyVisible &&
  //     (keyDownEvent.altKey || keyDownEvent.key === 'Alt') &&
  //     !keyDownEvent.ctrlKey) {

  //     this._areAccessKeyVisible = true;
  //     this._addAccessKeyOverlays();
  //   }
  // }

  // @HostListener('document:keyup', ['$event']) onKeyupHandler(keyUpEvent: KeyboardEvent) {
  //   if ((keyUpEvent.altKey || keyUpEvent.key === 'Alt' || keyUpEvent.key === 'Tab') && !keyUpEvent.ctrlKey) {
  //     this._hideAccessKeyOverlay();
  //     if (keyUpEvent.preventDefault) keyUpEvent.preventDefault(); else keyUpEvent.returnValue = false;
  //     if (keyUpEvent.stopPropagation) keyUpEvent.stopPropagation();
  //     if (keyUpEvent.cancelBubble) keyUpEvent.cancelBubble = true;
  //     return false;
  //   }
  // }

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
}
