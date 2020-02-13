import { Component, AfterViewInit, HostListener, ElementRef } from '@angular/core';
import { AuthService } from './auth/auth.service';
import { Router } from '@angular/router';
import { PrintService } from './print.service';
declare var $: any;
import * as _ from 'lodash';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  title = 'tmt-dental';
  _areAccessKeyVisible = false;

  constructor(public authService: AuthService, private router: Router, public printService: PrintService,
    private el: ElementRef) {
    this.loadGroups();
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
    this.authService.currentUser.subscribe(user => {
      if (user) {
        this.authService.getGroups().subscribe(result => {
          localStorage.setItem('groups', JSON.stringify(result));
        });
      }
    });
  }
}
