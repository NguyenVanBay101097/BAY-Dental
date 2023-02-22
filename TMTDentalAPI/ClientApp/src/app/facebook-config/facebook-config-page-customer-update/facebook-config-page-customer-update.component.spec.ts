import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { FacebookConfigPageCustomerUpdateComponent } from './facebook-config-page-customer-update.component';

describe('FacebookConfigPageCustomerUpdateComponent', () => {
  let component: FacebookConfigPageCustomerUpdateComponent;
  let fixture: ComponentFixture<FacebookConfigPageCustomerUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ FacebookConfigPageCustomerUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(FacebookConfigPageCustomerUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
