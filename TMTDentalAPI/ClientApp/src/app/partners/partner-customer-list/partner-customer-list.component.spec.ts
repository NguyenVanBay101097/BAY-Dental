import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerCustomerListComponent } from './partner-customer-list.component';

describe('PartnerCustomerListComponent', () => {
  let component: PartnerCustomerListComponent;
  let fixture: ComponentFixture<PartnerCustomerListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerCustomerListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerCustomerListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
