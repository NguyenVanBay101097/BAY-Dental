import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceCardCardHistoriesComponent } from './service-card-card-histories.component';

describe('ServiceCardCardHistoriesComponent', () => {
  let component: ServiceCardCardHistoriesComponent;
  let fixture: ComponentFixture<ServiceCardCardHistoriesComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceCardCardHistoriesComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceCardCardHistoriesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
