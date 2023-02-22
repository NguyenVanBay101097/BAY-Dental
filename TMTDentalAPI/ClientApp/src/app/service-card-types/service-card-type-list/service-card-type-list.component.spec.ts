import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceCardTypeListComponent } from './service-card-type-list.component';

describe('ServiceCardTypeListComponent', () => {
  let component: ServiceCardTypeListComponent;
  let fixture: ComponentFixture<ServiceCardTypeListComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceCardTypeListComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceCardTypeListComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
