import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookPageMarketingCustomerListComponent } from './facebook-page-marketing-customer-list.component';

describe('FacebookPageMarketingCustomerListComponent', () => {
  let component: FacebookPageMarketingCustomerListComponent;
  let fixture: ComponentFixture<FacebookPageMarketingCustomerListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookPageMarketingCustomerListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookPageMarketingCustomerListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
