import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { PartnerExistListDialogComponent } from './partner-exist-list-dialog.component';

describe('PartnerExistListDialogComponent', () => {
  let component: PartnerExistListDialogComponent;
  let fixture: ComponentFixture<PartnerExistListDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ PartnerExistListDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(PartnerExistListDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
