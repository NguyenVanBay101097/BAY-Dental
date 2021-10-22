import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceCardCardsManagementComponent } from './service-card-cards-management.component';

describe('ServiceCardCardsManagementComponent', () => {
  let component: ServiceCardCardsManagementComponent;
  let fixture: ComponentFixture<ServiceCardCardsManagementComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceCardCardsManagementComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceCardCardsManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
