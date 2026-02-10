import { Component } from '@angular/core';
import { Performer } from '../../Models/Performer ';
import { GenericService } from '../../Services/generic-service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-performer-component',
  imports: [CommonModule, FormsModule],
  templateUrl: './performer-component.html',
  styleUrl: './performer-component.css',
})
export class PerformerComponent {
  performers: Performer[] = [];
  newPerformer: Performer = new Performer();
  selectedPerformer: Performer | null = null;
  showForm = false;

  constructor(private performerService: GenericService<Performer>) {}

  ngOnInit() {
    this.loadPerformers();
  }

  loadPerformers() {
    this.performerService
      .getAll('Performer')
      .subscribe(res => (this.performers = res));
  }

  toggleForm() {
    this.showForm = !this.showForm;

    if (!this.showForm) {
      this.cancelEdit();
    } else if (!this.selectedPerformer) {
      this.newPerformer = new Performer();
    }
  }

  edit(p: Performer) {
    this.selectedPerformer = { ...p };
    this.newPerformer = { ...p };
    this.showForm = true;
  }

  save() {
    if (!this.newPerformer.name) return;

    // ✅ VERY IMPORTANT: always send id on update
    if (this.selectedPerformer?.id) {
      this.performerService
        .put('Performer', this.selectedPerformer.id, this.newPerformer)
        .subscribe(() => {
          this.loadPerformers();
          this.cancelEdit();
        });
    } else {
      this.performerService
        .post('Performer', this.newPerformer)
        .subscribe(() => {
          this.loadPerformers();
          this.toggleForm();
        });
    }
  }

  cancelEdit() {
    this.selectedPerformer = null;
    this.newPerformer = new Performer();
    this.showForm = false;
  }

  delete(id: number) {
    if (!confirm('Delete performer?')) return;
    this.performerService
      .delete('Performer', id)
      .subscribe(() => this.loadPerformers());
  }

}
