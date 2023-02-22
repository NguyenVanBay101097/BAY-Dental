import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceCardOrderCreateUpdateComponent } from './service-card-order-create-update.component';

describe('ServiceCardOrderCreateUpdateComponent', () => {
  let component: ServiceCardOrderCreateUpdateComponent;
  let fixture: ComponentFixture<ServiceCardOrderCreateUpdateComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceCardOrderCreateUpdateComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceCardOrderCreateUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
