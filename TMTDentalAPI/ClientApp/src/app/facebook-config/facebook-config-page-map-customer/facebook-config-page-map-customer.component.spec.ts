import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookConfigPageMapCustomerComponent } from './facebook-config-page-map-customer.component';

describe('FacebookConfigPageMapCustomerComponent', () => {
  let component: FacebookConfigPageMapCustomerComponent;
  let fixture: ComponentFixture<FacebookConfigPageMapCustomerComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookConfigPageMapCustomerComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookConfigPageMapCustomerComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
