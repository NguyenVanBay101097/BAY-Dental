import { Component, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { FacebookUserProfileService } from '../shared/facebook-user-profile.service';
import { FacebookUserProfileDisplay } from '../shared/facebook-user-profile-display';

@Component({
  selector: 'app-facebook-config-page-customer-update',
  templateUrl: './facebook-config-page-customer-update.component.html',
  styleUrls: ['./facebook-config-page-customer-update.component.css']
})
export class FacebookConfigPageCustomerUpdateComponent implements OnInit, OnChanges {

  ngOnChanges(changes: SimpleChanges): void {
    this.loadUserProfile();
  }

  @Input() conversation: any;
  profileDisplay: FacebookUserProfileDisplay;

  constructor(private userProfileService: FacebookUserProfileService) { }

  ngOnInit() {
  }

  loadUserProfile() {
    if (this.conversation) {
      this.userProfileService.get(this.conversation.userId).subscribe(result => {
        this.profileDisplay = result;
      });
    }
  }

  onPhoneChange(event) {
    console.log(this.profileDisplay);
  }
}
