import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { DotKhamCreateUpdateDialogComponent } from './dot-kham-create-update-dialog.component';

describe('DotKhamCreateUpdateDialogComponent', () => {
  let component: DotKhamCreateUpdateDialogComponent;
  let fixture: ComponentFixture<DotKhamCreateUpdateDialogComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DotKhamCreateUpdateDialogComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DotKhamCreateUpdateDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
