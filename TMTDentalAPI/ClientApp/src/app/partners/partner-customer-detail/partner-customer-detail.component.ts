import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { PartnerCustomerCuDialogComponent } from '../partner-customer-cu-dialog/partner-customer-cu-dialog.component';

@Component({
  selector: 'app-partner-customer-detail',
  templateUrl: './partner-customer-detail.component.html',
  styleUrls: ['./partner-customer-detail.component.css']
})
export class PartnerCustomerDetailComponent implements OnInit {

  id: string;
  eventSave: any;
  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private modalService: NgbModal
  ) { }

  ngOnInit() {
    this.id = this.route.snapshot.params['id'];
    if (this.id) {
      this.router.navigateByUrl(`customer/${this.id}/profile`);
    }
  }



  redirectUrl(event, value) {
    var eventProfile = document.getElementById('profile');
    if (eventProfile && eventProfile != event && !this.eventSave) {
      eventProfile.classList.remove('active-tab');
      event.currentTarget.classList.add('active-tab');
      this.eventSave = event;
    }
    else {
      this.eventSave.target.classList.remove('active-tab');
      event.currentTarget.classList.add('active-tab');
      this.eventSave = event;
    }

    this.router.navigateByUrl(`customer/${this.id}/${value}`);
  }
}
