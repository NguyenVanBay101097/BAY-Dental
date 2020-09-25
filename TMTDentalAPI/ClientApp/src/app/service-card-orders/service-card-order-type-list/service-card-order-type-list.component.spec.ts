import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceCardOrderTypeListComponent } from './service-card-order-type-list.component';

describe('ServiceCardOrderTypeListComponent', () => {
  let component: ServiceCardOrderTypeListComponent;
  let fixture: ComponentFixture<ServiceCardOrderTypeListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceCardOrderTypeListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceCardOrderTypeListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
