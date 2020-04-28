import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceCardCardListComponent } from './service-card-card-list.component';

describe('ServiceCardCardListComponent', () => {
  let component: ServiceCardCardListComponent;
  let fixture: ComponentFixture<ServiceCardCardListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceCardCardListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceCardCardListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
