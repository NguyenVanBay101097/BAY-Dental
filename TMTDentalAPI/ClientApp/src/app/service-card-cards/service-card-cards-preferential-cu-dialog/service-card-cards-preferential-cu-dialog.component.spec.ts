import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceCardCardsPreferentialCuDialogComponent } from './service-card-cards-preferential-cu-dialog.component';

describe('ServiceCardCardsPreferentialCuDialogComponent', () => {
  let component: ServiceCardCardsPreferentialCuDialogComponent;
  let fixture: ComponentFixture<ServiceCardCardsPreferentialCuDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceCardCardsPreferentialCuDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceCardCardsPreferentialCuDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
