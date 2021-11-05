import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ServiceCardCardsPreferentialImportDialogComponent } from './service-card-cards-preferential-import-dialog.component';

describe('ServiceCardCardsPreferentialImportDialogComponent', () => {
  let component: ServiceCardCardsPreferentialImportDialogComponent;
  let fixture: ComponentFixture<ServiceCardCardsPreferentialImportDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ServiceCardCardsPreferentialImportDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ServiceCardCardsPreferentialImportDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
