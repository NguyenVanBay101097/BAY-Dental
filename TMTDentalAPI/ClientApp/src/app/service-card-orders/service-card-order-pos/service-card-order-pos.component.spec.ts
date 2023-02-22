import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceCardOrderPosComponent } from './service-card-order-pos.component';

describe('ServiceCardOrderPosComponent', () => {
  let component: ServiceCardOrderPosComponent;
  let fixture: ComponentFixture<ServiceCardOrderPosComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceCardOrderPosComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceCardOrderPosComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
