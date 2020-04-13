import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookPageMarketingCustomerDialogComponent } from './facebook-page-marketing-customer-dialog.component';

describe('FacebookPageMarketingCustomerDialogComponent', () => {
  let component: FacebookPageMarketingCustomerDialogComponent;
  let fixture: ComponentFixture<FacebookPageMarketingCustomerDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookPageMarketingCustomerDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookPageMarketingCustomerDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
