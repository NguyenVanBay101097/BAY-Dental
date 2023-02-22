import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceCardOrderListComponent } from './service-card-order-list.component';

describe('ServiceCardOrderListComponent', () => {
  let component: ServiceCardOrderListComponent;
  let fixture: ComponentFixture<ServiceCardOrderListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceCardOrderListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceCardOrderListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
